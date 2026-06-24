using EducationalPlatformDesktop.Api.Contracts;
using System.Net.Http;
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
    }
}