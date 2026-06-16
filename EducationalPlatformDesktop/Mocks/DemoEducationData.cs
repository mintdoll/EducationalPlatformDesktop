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
                EmailOrMax = "arina@example.com",
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
                    Title = "Основы программирования",
                    Description = "Вводный курс по базовым принципам программирования.",
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
                                    Title = "Что такое программирование",
                                    Content = "Программирование - это процесс создания инструкций для компьютера. В этом уроке студент знакомится с базовыми понятиями, языками программирования и областью их применения."
                                },
                                new Lesson
                                {
                                    Title = "Алгоритмы и исполнители",
                                    Content = "Алгоритм - это последовательность действий, которая приводит к результату. На этом уроке рассматриваются линейные, ветвящиеся и циклические алгоритмы."
                                }
                            }
                        },
                        new Module
                        {
                            Title = "Модуль 2. Основы синтаксиса",
                            Lessons = new ObservableCollection<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "Переменные и типы данных",
                                    Content = "Урок посвящен переменным, типам данных и простым операциям над ними. Здесь объясняется, как хранить и изменять данные в программе."
                                },
                                new Lesson
                                {
                                    Title = "Операторы и выражения",
                                    Content = "В этом уроке рассматриваются арифметические, логические и сравнительные операторы, а также базовые выражения."
                                }
                            }
                        }
                    }
                },

                new Course
                {
                    Title = "WPF и MVVM",
                    Description = "Курс для изучения построения настольных приложений на WPF.",
                    IsPurchased = false,
                    Modules = new ObservableCollection<Module>
                    {
                        new Module
                        {
                            Title = "Модуль 1. Введение в WPF",
                            Lessons = new ObservableCollection<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "Что такое WPF",
                                    Content = "WPF - это технология для создания графических интерфейсов в Windows. Она позволяет строить красивые и удобные окна приложения."
                                },
                                new Lesson
                                {
                                    Title = "Разметка XAML",
                                    Content = "XAML используется для описания интерфейса. Здесь можно размещать кнопки, поля ввода, списки и другие элементы управления."
                                }
                            }
                        },
                        new Module
                        {
                            Title = "Модуль 2. Основы MVVM",
                            Lessons = new ObservableCollection<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "View, ViewModel, Model",
                                    Content = "MVVM разделяет интерфейс, логику представления и данные. Это помогает делать код чище и удобнее для сопровождения."
                                },
                                new Lesson
                                {
                                    Title = "Команды и привязки",
                                    Content = "Этот урок объясняет, как связывать интерфейс с логикой через команды и привязки данных."
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}