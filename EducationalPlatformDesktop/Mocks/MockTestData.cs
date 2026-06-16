using System.Collections.ObjectModel;
using EducationalPlatformDesktop.Models;

namespace EducationalPlatformDesktop.Mocks
{
    public static class MockTestData
    {
        public static Test GetTest()
        {
            return new Test
            {
                Title = "Тест: Основы программирования",
                Questions = new ObservableCollection<Question>
                {
                    new Question
                    {
                        Text = "Что такое алгоритм?",
                        Options = new ObservableCollection<string>
                        {
                            "Язык программирования",
                            "Последовательность действий для решения задачи",
                            "Тип данных",
                            "Среда разработки"
                        },
                        CorrectIndex = 1
                    },
                    new Question
                    {
                        Text = "Какой тип данных хранит целые числа в C#?",
                        Options = new ObservableCollection<string>
                        {
                            "string", "bool", "int", "double"
                        },
                        CorrectIndex = 2
                    },
                    new Question
                    {
                        Text = "Что делает оператор if?",
                        Options = new ObservableCollection<string>
                        {
                            "Повторяет код",
                            "Объявляет переменную",
                            "Проверяет условие и выполняет код",
                            "Выводит текст"
                        },
                        CorrectIndex = 2
                    }
                }
            };
        }

        public static ObservableCollection<Progress> GetProgress()
        {
            return new ObservableCollection<Progress>
            {
                new Progress
                {
                    CourseName = "Основы программирования",
                    CompletedLessons = 4,
                    TotalLessons = 4,
                    TestScore = 85,
                    IsCompleted = true
                },
                new Progress
                {
                    CourseName = "WPF и MVVM",
                    CompletedLessons = 2,
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
                    CourseName = "Основы программирования",
                    StudentName = "Арина Бутакова",
                    IssuedDate = "16.06.2026",
                    Score = 85
                }
            };
        }
    }
}