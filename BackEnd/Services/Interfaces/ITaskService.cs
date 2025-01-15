using Microsoft.AspNetCore.Mvc;
using TarikhMaghribi.Models.DTOs;
using task = TarikhMaghribi.Models.Entities.Task;
namespace TarikhMaghribi.Services.Interfaces
{
    public interface ITaskService
    {
        Task<IActionResult> GetTaskStatistics(string userId, int annee, int mois);
        Task<List<task>> GetUserTasksRecentToOld(string userId);
        Task<bool> CreateTask(string userId, TaskDto taskDto);
        Task<List<task>> GetTasksByMonthAndYear(string userId, int annee, int mois);
        Task<bool> UpdateTask(int id, string userId, TaskDto taskDto);
        Task<bool> DeleteTask(int id, string userId);
    }
}
