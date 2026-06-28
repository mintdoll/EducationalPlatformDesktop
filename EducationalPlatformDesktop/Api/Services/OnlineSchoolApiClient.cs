using EducationalPlatformDesktop.Api.Contracts;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace EducationalPlatformDesktop.Api.Services
{
    public sealed class OnlineSchoolApiClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;

        public OnlineSchoolApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(ApiConfig.BaseUrl);
        }

        public void SetBearerToken(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                return;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/Authentication/login",
                request,
                JsonOptions,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<LoginResponseDto>(JsonOptions, cancellationToken);
        }

        public async Task<List<CourseDto>> GetCoursesAsync(CancellationToken cancellationToken = default)
        {
            var courses = await _httpClient.GetFromJsonAsync<List<CourseDto>>(
                "api/Course",
                JsonOptions,
                cancellationToken);

            return courses ?? new List<CourseDto>();
        }

        public async Task<CourseDto?> GetCourseByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/Course/{id}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<CourseDto>(JsonOptions, cancellationToken);
        }

        public async Task<List<LessonDto>> GetLessonsByCourseIdAsync(int courseId, CancellationToken cancellationToken = default)
        {
            var lessons = await _httpClient.GetFromJsonAsync<List<LessonDto>>(
                $"api/Lesson/course/{courseId}",
                JsonOptions,
                cancellationToken);

            return lessons ?? new List<LessonDto>();
        }
        public async Task<ApiResult<List<TestDto>>> GetTestsByCourseIdAsync(
    int courseId,
    CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(
                $"api/Test/course/{courseId}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return ApiResult<List<TestDto>>.Error(
                    response.StatusCode,
                    GetFriendlyErrorMessage(response.StatusCode));
            }

            var tests = await response.Content.ReadFromJsonAsync<List<TestDto>>(
                JsonOptions,
                cancellationToken);

            return ApiResult<List<TestDto>>.Success(
                tests ?? new List<TestDto>(),
                response.StatusCode);
        }

        public async Task<ApiResult<TestFinishResponseDto>> FinishTestAsync(
            TestFinishRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/Test/finish",
                request,
                JsonOptions,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return ApiResult<TestFinishResponseDto>.Error(
                    response.StatusCode,
                    GetFriendlyErrorMessage(response.StatusCode));
            }

            var result = await response.Content.ReadFromJsonAsync<TestFinishResponseDto>(
                JsonOptions,
                cancellationToken);

            return ApiResult<TestFinishResponseDto>.Success(
                result ?? new TestFinishResponseDto(),
                response.StatusCode);
        }

        public async Task<ApiResult<List<UserCourseDto>>> GetUserCoursesAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(
                $"api/UserCourse/user/{userId}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return ApiResult<List<UserCourseDto>>.Error(
                    response.StatusCode,
                    GetFriendlyErrorMessage(response.StatusCode));
            }

            var courses = await response.Content.ReadFromJsonAsync<List<UserCourseDto>>(
                JsonOptions,
                cancellationToken);

            return ApiResult<List<UserCourseDto>>.Success(
                courses ?? new List<UserCourseDto>(),
                response.StatusCode);
        }

        public async Task<ApiResult<CertificateDto>> GetCertificateAsync(
            int userId,
            int courseId,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(
                $"api/Certificate/{userId}/{courseId}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return ApiResult<CertificateDto>.Error(
                    response.StatusCode,
                    GetFriendlyErrorMessage(response.StatusCode));
            }

            var certificate = await response.Content.ReadFromJsonAsync<CertificateDto>(
                JsonOptions,
                cancellationToken);

            return ApiResult<CertificateDto>.Success(
                certificate ?? new CertificateDto(),
                response.StatusCode);
        }

        private static string GetFriendlyErrorMessage(System.Net.HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                System.Net.HttpStatusCode.BadRequest =>
                    "Некорректный запрос. Проверьте данные и повторите попытку.",

                System.Net.HttpStatusCode.Unauthorized =>
                    "Сессия истекла или пользователь не авторизован. Войдите в систему заново.",

                System.Net.HttpStatusCode.NotFound =>
                    "Данные не найдены или действие недоступно для выбранного курса.",

                _ =>
                    "Не удалось получить данные от сервера. Попробуйте позже."
            };
        }
    }
}
