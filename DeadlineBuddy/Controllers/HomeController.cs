
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace DeadlineBuddy.Controllers
{
    public class HomeController : Controller
    {
        private const string SessionKey = "DeadlineTasks";

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddDeadline()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddDeadline(
            string Title,
            string Subject,
            DateTime DueDate,
            string Category,
            string Priority)
        {
            if (string.IsNullOrWhiteSpace(Title) ||
                string.IsNullOrWhiteSpace(Subject) ||
                string.IsNullOrWhiteSpace(Category) ||
                string.IsNullOrWhiteSpace(Priority) ||
                DueDate == default)
            {
                TempData["SuccessMessage"] = "Please complete all deadline fields.";
                return RedirectToAction("AddDeadline");
            }

            var deadlines = GetDeadlines();

            deadlines.Add(new Dictionary<string, string>
            {
                ["Title"] = Title.Trim(),
                ["Subject"] = Subject.Trim(),
                ["DueDate"] = DueDate.ToString("yyyy-MM-dd"),
                ["Category"] = Category,
                ["Priority"] = Priority,
                ["Completed"] = "false"
            });

            SaveDeadlines(deadlines);

            TempData["SuccessMessage"] = "Deadline added successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CompleteDeadline(string title)
        {
            var deadlines = GetDeadlines();

            var task = deadlines.FirstOrDefault(t =>
                t["Title"] == title && t["Completed"] == "false");

            if (task != null)
            {
                task["Completed"] = "true";
                SaveDeadlines(deadlines);
                TempData["SuccessMessage"] = "Task marked as completed!";
            }

            return RedirectToAction("Index");
        }

        private List<Dictionary<string, string>> GetDeadlines()
        {
            var json = HttpContext.Session.GetString(SessionKey);

            if (string.IsNullOrEmpty(json))
            {
                return new List<Dictionary<string, string>>();
            }

            return JsonSerializer.Deserialize<List<Dictionary<string, string>>>(json)
                   ?? new List<Dictionary<string, string>>();
        }

        private void SaveDeadlines(List<Dictionary<string, string>> deadlines)
        {
            HttpContext.Session.SetString(
                SessionKey,
                JsonSerializer.Serialize(deadlines));
        }
    }
}
