using System;
using System.Collections.ObjectModel;
using EducationalPlatformDesktop.Models;

namespace EducationalPlatformDesktop.Mocks
{
    public static class MockTestData
    {
        public static Test GetTestForCourse(int courseId)
        {
            return courseId switch
            {
                1 => CreatePythonTest(),
                2 => CreateAccountingTest(),
                3 => CreateLogisticsTest(),
                _ => throw new ArgumentException(
                    $"Для курса с идентификатором {courseId} тест не найден.",
                    nameof(courseId))
            };
        }

        // Временная совместимость со старым MainViewModel.
        // На следующем этапе MainViewModel будет работать через Course.Id.
        public static Test GetTest(string courseName)
        {
            var courseId = courseName switch
            {
                "Python для начинающих" => 1,
                "Бухгалтерский учёт" => 2,
                "Логистика и закупки" => 3,
                _ => 1
            };

            return GetTestForCourse(courseId);
        }

        public static ObservableCollection<Progress> GetProgress()
        {
            return new ObservableCollection<Progress>
            {
                new Progress
                {
                    CourseName = "Python для начинающих",
                    CompletedLessons = 3,
                    TotalLessons = 4,
                    TestScore = 85,
                    IsCompleted = false
                },
                new Progress
                {
                    CourseName = "Бухгалтерский учёт",
                    CompletedLessons = 4,
                    TotalLessons = 4,
                    TestScore = 85,
                    IsCompleted = true
                },
                new Progress
                {
                    CourseName = "Логистика и закупки",
                    CompletedLessons = 0,
                    TotalLessons = 4,
                    TestScore = 0,
                    IsCompleted = false
                }
            };
        }

        public static ObservableCollection<Certificate> GetCertificates()
        {
            return new ObservableCollection<Certificate>
    {
        new Certificate
        {
            Id = "certificate-accounting-demo",
            Number = "CERT-20260616-ACC001",
            CourseId = 2,
            CourseName = "Бухгалтерский учёт",
            StudentName = "Арина Бутакова",
            IssuedAt = new DateTime(2026, 6, 16),
            Score = 85,
            TestResultId = "demo-accounting-result"
        }
    };
        }
        private static Test CreatePythonTest()
        {
            const int testId = 101;

            return new Test
            {
                Id = testId,
                CourseId = 1,
                Title = "Итоговый тест · Python для начинающих",
                PassingScore = 70,
                Questions = new ObservableCollection<Question>
                {
                    CreateQuestion(
                        id: 1001,
                        testId: testId,
                        text: "Для чего в Python используются переменные?",
                        correctIndex: 1,
                        "Для оформления комментариев",
                        "Для хранения данных",
                        "Только для вывода текста",
                        "Для установки Python"),

                    CreateQuestion(
                        id: 1002,
                        testId: testId,
                        text: "Какой тип данных хранит целые числа?",
                        correctIndex: 2,
                        "str",
                        "bool",
                        "int",
                        "float"),

                    CreateQuestion(
                        id: 1003,
                        testId: testId,
                        text: "Что делает оператор if?",
                        correctIndex: 2,
                        "Повторяет код бесконечно",
                        "Создаёт новую функцию",
                        "Проверяет условие",
                        "Завершает программу")
                }
            };
        }

        private static Test CreateAccountingTest()
        {
            const int testId = 102;

            return new Test
            {
                Id = testId,
                CourseId = 2,
                Title = "Итоговый тест · Бухгалтерский учёт",
                PassingScore = 70,
                Questions = new ObservableCollection<Question>
                {
                    CreateQuestion(
                        id: 2001,
                        testId: testId,
                        text: "Для чего нужен бухгалтерский учёт?",
                        correctIndex: 1,
                        "Только для расчёта зарплаты",
                        "Для регистрации и обобщения информации о деятельности организации",
                        "Только для хранения договоров",
                        "Для создания рекламы"),

                    CreateQuestion(
                        id: 2002,
                        testId: testId,
                        text: "Что подтверждает хозяйственную операцию?",
                        correctIndex: 2,
                        "Устная договорённость",
                        "Рекламное объявление",
                        "Первичный документ",
                        "Рабочий график"),

                    CreateQuestion(
                        id: 2003,
                        testId: testId,
                        text: "Какие элементы используются в бухгалтерской проводке?",
                        correctIndex: 0,
                        "Дебет и кредит",
                        "Доход и реклама",
                        "Склад и доставка",
                        "План и отчёт")
                }
            };
        }

        private static Test CreateLogisticsTest()
        {
            const int testId = 103;

            return new Test
            {
                Id = testId,
                CourseId = 3,
                Title = "Итоговый тест · Логистика и закупки",
                PassingScore = 70,
                Questions = new ObservableCollection<Question>
                {
                    CreateQuestion(
                        id: 3001,
                        testId: testId,
                        text: "Что изучает логистика?",
                        correctIndex: 1,
                        "Только бухгалтерские документы",
                        "Движение товаров, информации и ресурсов",
                        "Только работу персонала",
                        "Разработку программ"),

                    CreateQuestion(
                        id: 3002,
                        testId: testId,
                        text: "Зачем необходимо планирование закупок?",
                        correctIndex: 2,
                        "Для увеличения количества документов",
                        "Для изменения названия организации",
                        "Для предотвращения дефицита и излишков",
                        "Только для выбора транспорта"),

                    CreateQuestion(
                        id: 3003,
                        testId: testId,
                        text: "Что учитывают при выборе поставщика?",
                        correctIndex: 3,
                        "Только название компании",
                        "Только адрес офиса",
                        "Только количество сотрудников",
                        "Цену, сроки и качество поставки")
                }
            };
        }

        private static Question CreateQuestion(
            int id,
            int testId,
            string text,
            int correctIndex,
            params string[] optionTexts)
        {
            var question = new Question
            {
                Id = id,
                TestId = testId,
                Text = text
            };

            for (var index = 0; index < optionTexts.Length; index++)
            {
                question.Options.Add(new AnswerOption
                {
                    Id = id * 10 + index + 1,
                    QuestionId = id,
                    Text = optionTexts[index],
                    IsCorrect = index == correctIndex
                });
            }

            return question;
        }
    }
}