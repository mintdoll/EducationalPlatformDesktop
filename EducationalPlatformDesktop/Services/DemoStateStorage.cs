using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using EducationalPlatformDesktop.Models;

namespace EducationalPlatformDesktop.Services
{
    public class DemoStateStorage
    {
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private string StateDirectory =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EducationalPlatformDesktop");

        private string StateFilePath =>
            Path.Combine(StateDirectory, "demo-state.json");

        public DemoAppState? Load()
        {
            try
            {
                if (!File.Exists(StateFilePath))
                {
                    return null;
                }

                var json = File.ReadAllText(StateFilePath);

                return JsonSerializer.Deserialize<DemoAppState>(
                    json,
                    _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        public void Save(DemoAppState state)
        {
            Directory.CreateDirectory(StateDirectory);

            var json = JsonSerializer.Serialize(
                state,
                _jsonOptions);

            File.WriteAllText(StateFilePath, json);
        }
    }
}