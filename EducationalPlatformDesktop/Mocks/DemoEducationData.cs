using System.Collections.ObjectModel;
using EducationalPlatformDesktop.Models;

namespace EducationalPlatformDesktop.Mocks
{
    public static class DemoEducationData
    {
        public static ObservableCollection<Course> GetCourses()
        {
            return new ObservableCollection<Course>
            {
                new Course
                {
                    Title = "Python для начинающих",
                    Track = "IT",
                    Teacher = "Петров П.П.",
                    Description = "Основы программирования, типы данных, условия, циклы и функции.",
                    Progress = 68,
                    IsPurchased = true,
                    Modules = new ObservableCollection<Module>
                    {
                        new Module
                        {
                            Title = "Модуль 1. Введение",
                            Lessons = new ObservableCollection<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "Что такое Python",
                                    DurationMinutes = 10,
                                    Content = "Python - это язык программирования общего назначения. Он прост для изучения и часто используется для автоматизации, анализа данных и создания приложений."
                                },
                                new Lesson
                                {
                                    Title = "Установка среды",
                                    DurationMinutes = 12,
                                    Content = "Для работы с Python нужно установить интерпретатор и среду разработки. На этом уроке студент знакомится с тем, как подготовить рабочее окружение."
                                }
                            }
                        },
                        new Module
                        {
                            Title = "Модуль 2. Базовый синтаксис",
                            Lessons = new ObservableCollection<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "Переменные",
                                    DurationMinutes = 14,
                                    Content = "Переменные используются для хранения данных. В Python они создаются без явного указания типа, что делает язык удобным для новичков."
                                },
                                new Lesson
                                {
                                    Title = "Условия",
                                    DurationMinutes = 16,
                                    Content = "Условные конструкции позволяют программе принимать решения. В Python для этого используется оператор if."
                                }
                            }
                        },
                        new Module
                        {
                            Title = "Модуль 3. Функции",
                            Lessons = new ObservableCollection<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "Определение функций",
                                    DurationMinutes = 12,
                                    Content = "Функции помогают разбивать программу на небольшие логические части. Это упрощает поддержку и повторное использование кода."
                                },
                                new Lesson
                                {
                                    Title = "Аргументы и return",
                                    DurationMinutes = 18,
                                    Content = "Аргументы позволяют передавать данные в функцию, а оператор return возвращает результат её работы."
                                }
                            }
                        }
                    }
                },

                new Course
                {
                    Title = "Бухгалтерский учёт",
                    Track = "Бухгалтерия",
                    Teacher = "Иванова А.С.",
                    Description = "Документы, счета, проводки, отчётность и основы финансового учёта.",
                    Progress = 30,
                    IsPurchased = true,
                    Modules = new ObservableCollection<Module>
                    {
                        new Module
                        {
                            Title = "Модуль 1. Основы учета",
                            Lessons = new ObservableCollection<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "Что такое бухгалтерия",
                                    DurationMinutes = 10,
                                    Content = "Бухгалтерский учет - это система сбора, регистрации и обобщения информации о деятельности организации."
                                },
                                new Lesson
                                {
                                    Title = "Объекты учета",
                                    DurationMinutes = 14,
                                    Content = "К объектам учета относятся имущество, обязательства, доходы и расходы организации."
                                }
                            }
                        },
                        new Module
                        {
                            Title = "Модуль 2. Проводки",
                            Lessons = new ObservableCollection<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "Дебет и кредит",
                                    DurationMinutes = 12,
                                    Content = "Бухгалтерские проводки строятся на принципе двойной записи. Каждая операция отражается по дебету одного счёта и кредиту другого."
                                },
                                new Lesson
                                {
                                    Title = "Примеры проводок",
                                    DurationMinutes = 17,
                                    Content = "На этом уроке разбираются простые примеры проводок и их отражение в учёте."
                                }
                            }
                        }
                    }
                },

                new Course
                {
                    Title = "Логистика и закупки",
                    Track = "Логистика",
                    Teacher = "Кузнецов Д.В.",
                    Description = "Логистические процессы, снабжение, склад и транспортная цепочка.",
                    Progress = 12,
                    IsPurchased = false,
                    Modules = new ObservableCollection<Module>
                    {
                        new Module
                        {
                            Title = "Модуль 1. Введение в логистику",
                            Lessons = new ObservableCollection<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "Роль логистики",
                                    DurationMinutes = 11,
                                    Content = "Логистика отвечает за планирование, организацию и контроль движения товаров, информации и ресурсов."
                                },
                                new Lesson
                                {
                                    Title = "Цепочка поставок",
                                    DurationMinutes = 15,
                                    Content = "Цепочка поставок объединяет поставщиков, склады, транспорт и конечного потребителя."
                                }
                            }
                        },
                        new Module
                        {
                            Title = "Модуль 2. Закупки",
                            Lessons = new ObservableCollection<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "Выбор поставщика",
                                    DurationMinutes = 12,
                                    Content = "Выбор поставщика включает анализ условий, сроков, качества и стоимости поставок."
                                },
                                new Lesson
                                {
                                    Title = "Складская логика",
                                    DurationMinutes = 18,
                                    Content = "Складская логика помогает организовать хранение, учёт и отгрузку товаров."
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}