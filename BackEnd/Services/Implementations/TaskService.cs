using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TarikhMaghribi.DBContext;
using TarikhMaghribi.Models.DTOs;
using TarikhMaghribi.Services.Interfaces;
using task = TarikhMaghribi.Models.Entities.Task;

namespace TarikhMaghribi.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetTaskStatistics(string userId, int annee, int mois)
        {
            try
            {
                var tasksInMonth = await _context.Tasks
                    .Where(t => t.UserId == userId && t.Date.Year == annee && t.Date.Month == mois)
                    .ToListAsync();

                var totalTasks = tasksInMonth.Count;
                var notStartedTasks = tasksInMonth.Count(t => t.Status == "pas encore");
                var inProgressTasks = tasksInMonth.Count(t => t.Status == "encore");
                var completedTasks = tasksInMonth.Count(t => t.Status == "terminé");

                var holidayCountMonth = await _context.JoursFeries
                    .Where(h => h.DateJour.Year == annee && h.DateJour.Month == mois)
                    .CountAsync();
                var holidayCountTotalYear = await _context.JoursFeries
                    .Where(h => h.DateJour.Year == annee)
                    .CountAsync();

                var result = new
                {
                    TotalTasks = totalTasks,
                    NotStartedTasks = notStartedTasks,
                    InProgressTasks = inProgressTasks,
                    CompletedTasks = completedTasks,
                    holidayCountByMonth = holidayCountMonth,
                    holidayCountTotalByYear = holidayCountTotalYear
                };

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }
        public async Task<List<task>> GetUserTasksRecentToOld(string userId)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<bool> CreateTask(string userId, TaskDto taskDto)
        {
            var task = new task
            {
                Date = taskDto.Date,
                Title = taskDto.Title,
                Status = taskDto.Status,
                UserId = userId
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<task>> GetTasksByMonthAndYear(string userId, int annee, int mois)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId && t.Date.Year == annee && t.Date.Month == mois)
                .OrderBy(t => t.Date)
                .ToListAsync();
        }

        public async Task<bool> UpdateTask(int id, string userId, TaskDto taskDto)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null || task.UserId != userId)
            {
                return false;
            }

            task.Date = taskDto.Date;
            task.Title = taskDto.Title;
            task.Status = taskDto.Status;

            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTask(int id, string userId)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null || task.UserId != userId)
            {
                return false;
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
