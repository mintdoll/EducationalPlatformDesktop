using System;
using System.Collections.Generic;
using System.Text;

namespace EducationalPlatformDesktop.Models
{
    public class AnswerOption
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public string Text { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }
    }
}