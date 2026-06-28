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
                1 => CreateWebDevelopmentTest(),
                2 => CreatePythonTest(),
                3 => CreateFigmaTest(),
                _ => CreateGenericTest(courseId)
            };
        }

        private static Test CreateWebDevelopmentTest()
        {
            const int testId = 101;

            return new Test
            {
                Id = testId,
                CourseId = 1,
                Title = "Итоговый тест · Веб-разработка с нуля",
                PassingScore = 70,
                Questions = new ObservableCollection<Question>
                {
                    CreateQuestion(
                        id: 1001,
                        testId: testId,
                        text: "Какой тег HTML используется для создания гиперссылки?",
                        correctIndex: 1,
                        "<link>",
                        "<a>",
                        "<href>",
                        "<url>"),
                    CreateQuestion(
                        id: 1002,
                        testId: testId,
                        text: "Что означает аббревиатура CSS?",
                        correctIndex: 2,
                        "Computer Style Sheets",
                        "Creative Style System",
                        "Cascading Style Sheets",
                        "Coded Styling Syntax"),
                    CreateQuestion(
                        id: 1003,
                        testId: testId,
                        text: "Какой HTTP-метод используется для отправки данных формы на сервер?",
                        correctIndex: 1,
                        "GET",
                        "POST",
                        "PUT",
                        "DELETE")
                }
            };
        }

        private static Test CreatePythonTest()
        {
            const int testId = 102;

            return new Test
            {
                Id = testId,
                CourseId = 2,
                Title = "Итоговый тест · Основы Python",
                PassingScore = 70,
                Questions = new ObservableCollection<Question>
                {
                    CreateQuestion(
                        id: 2001,
                        testId: testId,
                        text: "Как объявить функцию в Python?",
                        correctIndex: 2,
                        "function my_func():",
                        "func my_func():",
                        "def my_func():",
                        "define my_func():"),
                    CreateQuestion(
                        id: 2002,
                        testId: testId,
                        text: "Какой тип данных используется для хранения пар «ключ — значение» в Python?",
                        correctIndex: 3,
                        "list",
                        "tuple",
                        "set",
                        "dict"),
                    CreateQuestion(
                        id: 2003,
                        testId: testId,
                        text: "Какой оператор проверяет принадлежность элемента коллекции?",
                        correctIndex: 2,
                        "contains",
                        "has",
                        "in",
                        "includes")
                }
            };
        }

        private static Test CreateFigmaTest()
        {
            const int testId = 103;

            return new Test
            {
                Id = testId,
                CourseId = 3,
                Title = "Итоговый тест · Дизайн интерфейсов в Figma",
                PassingScore = 70,
                Questions = new ObservableCollection<Question>
                {
                    CreateQuestion(
                        id: 3001,
                        testId: testId,
                        text: "Что такое компонент (Component) в Figma?",
                        correctIndex: 1,
                        "Слой с примененным эффектом",
                        "Переиспользуемый элемент интерфейса, изменения в котором применяются ко всем копиям",
                        "Группа слоев без особых свойств",
                        "Шаблон страницы"),
                    CreateQuestion(
                        id: 3002,
                        testId: testId,
                        text: "Для чего используется Auto Layout?",
                        correctIndex: 2,
                        "Для автоматической генерации цветовой палитры",
                        "Для создания анимации между экранами",
                        "Для адаптивного выравнивания элементов с автоматическими отступами",
                        "Для экспорта макета в код"),
                    CreateQuestion(
                        id: 3003,
                        testId: testId,
                        text: "Как называется копия компонента, размещенная на макете?",
                        correctIndex: 2,
                        "Clone",
                        "Symbol",
                        "Instance",
                        "Draft")
                }
            };
        }

        private static Test CreateGenericTest(int courseId)
        {
            var testId = 1000 + courseId;

            return new Test
            {
                Id = testId,
                CourseId = courseId,
                Title = "Итоговый тест",
                PassingScore = 70,
                Questions = new ObservableCollection<Question>
                {
                    CreateQuestion(
                        id: testId * 10 + 1,
                        testId: testId,
                        text: "Выберите один правильный ответ.",
                        correctIndex: 0,
                        "Правильный ответ",
                        "Неверный ответ",
                        "Неверный ответ",
                        "Неверный ответ"),
                    CreateQuestion(
                        id: testId * 10 + 2,
                        testId: testId,
                        text: "Какое действие завершает тест?",
                        correctIndex: 1,
                        "Пропуск вопроса",
                        "Отправка результата",
                        "Перезагрузка окна",
                        "Смена темы")
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
