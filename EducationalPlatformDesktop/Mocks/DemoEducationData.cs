using System.Collections.ObjectModel;
using EducationalPlatformDesktop.Models;

namespace EducationalPlatformDesktop.Mocks
{
    public static class DemoEducationData
    {
        public static UserProfile GetProfile()
        {
            return new UserProfile
            {
                FullName = "Арина Бутакова",
                EmailOrMax = "arina.butakova@example.com",
                Group = "БИ-24-1",
                Role = "Студент"
            };
        }

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
                    Progress = 85,
                    CompletedLessons = 3,
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
                                    Content =
@"Python — это язык программирования общего назначения, который подходит для автоматизации, анализа данных, веб-разработки и обучения программированию.

На этом уроке рассматриваются:
- что такое язык программирования;
- где применяется Python;
- чем Python отличается от других языков.

Главная идея Python — простота синтаксиса и читаемость кода."
                                },
                                new Lesson
                                {
                                    Title = "Установка среды",
                                    DurationMinutes = 12,
                                    Content =
@"Перед началом работы с Python нужно установить сам интерпретатор и редактор кода.

В рамках урока:
- как установить Python;
- как проверить установку;
- как создать первый файл .py;
- как запустить программу.

После этого можно переходить к написанию простых скриптов."
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
                                    Title = "Переменные и типы данных",
                                    DurationMinutes = 15,
                                    Content =
@"Переменные нужны для хранения данных.

В Python используются:
- числа;
- строки;
- логические значения;
- списки;
- словари.

На этом уроке разбирается, как создавать переменные и как понимать типы данных."
                                },
                                new Lesson
                                {
                                    Title = "Условия и циклы",
                                    DurationMinutes = 18,
                                    Content =
@"Условия позволяют программе принимать решения, а циклы — повторять действия.

В уроке изучаются:
- if, elif, else;
- for;
- while;
- примеры простых сценариев.

Это основа любой логики в программе."
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
                    Progress = 100,
                    CompletedLessons = 4,
                    IsPurchased = true,
                    Modules = new ObservableCollection<Module>
                    {
                        new Module
                        {
                            Title = "Модуль 1. Основы учёта",
                            Lessons = new ObservableCollection<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "Что такое бухгалтерский учёт",
                                    DurationMinutes = 11,
                                    Content =
@"Бухгалтерский учёт — это система сбора, регистрации и обобщения информации о деятельности организации.

На уроке рассматриваются:
- назначение учёта;
- роль бухгалтера;
- основные термины и принципы."
                                },
                                new Lesson
                                {
                                    Title = "Документы и первичные операции",
                                    DurationMinutes = 14,
                                    Content =
@"Первичные документы подтверждают хозяйственные операции.

На уроке разбираются:
- накладные;
- акты;
- счета;
- платежные документы;
- зачем нужны документы в учёте."
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
                                    DurationMinutes = 16,
                                    Content =
@"Дебет и кредит — базовые элементы бухгалтерской записи.

На этом уроке:
- что означает каждая сторона проводки;
- как читать проводки;
- простые примеры отражения операций."
                                },
                                new Lesson
                                {
                                    Title = "Простые проводки",
                                    DurationMinutes = 17,
                                    Content =
@"Простые проводки используются для отражения основных хозяйственных операций.

Разбираются примеры:
- поступление денег;
- оплата поставщику;
- списание материалов;
- отражение продажи."
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
                    Progress = 0,
                    CompletedLessons = 0,
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
                                    Title = "Что такое логистика",
                                    DurationMinutes = 9,
                                    Content =
@"Логистика изучает движение товаров, информации и ресурсов от поставщика к потребителю.

На уроке:
- что входит в логистику;
- какие бывают цепочки поставок;
- зачем нужна логистика в компании."
                                },
                                new Lesson
                                {
                                    Title = "Склад и хранение",
                                    DurationMinutes = 13,
                                    Content =
@"Склад — важная часть логистической системы.

Рассматриваются:
- хранение товаров;
- учёт на складе;
- движение материалов;
- роль складской инфраструктуры."
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
                                    Title = "Планирование закупок",
                                    DurationMinutes = 12,
                                    Content =
@"Планирование закупок помогает компании избежать дефицита и излишков.

На уроке:
- зачем нужен план закупок;
- как рассчитывают потребность;
- как формируется заявка."
                                },
                                new Lesson
                                {
                                    Title = "Работа с поставщиками",
                                    DurationMinutes = 15,
                                    Content =
@"Работа с поставщиками включает выбор контрагента, согласование условий и контроль поставки.

Разбираются:
- критерии выбора поставщика;
- договорные условия;
- сроки и качество поставок."
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}