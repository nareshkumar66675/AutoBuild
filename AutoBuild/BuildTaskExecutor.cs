using AutoBuild.Helper;
using BMC.Common.LogManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoBuild
{
    enum TaskStatus
    {
        [Description("Completed")]
        Completed,
        [Description("Failed")]
        Failed,
        [Description("Not Started")]
        NotStarted
    }
    class TaskDictionary<T>
    {
        public TaskDictionary(TaskInfo _taskInfo,Task<T> _Task)
        {
            this.TaskInfo = _taskInfo;
            this.Task = _Task;
            this.Status = TaskStatus.NotStarted;
        }
        public TaskInfo TaskInfo { get; set; }
        public Task<T> Task { get; set; }
        public TaskStatus Status { get; set; }
    }
    class BuildTaskExecutor
    {
        public static List<TaskDictionary<int>> TaskList = new List<TaskDictionary<int>>();
        /// <summary>
        /// Executes the a Task.
        /// </summary>
        /// <param name="taskId">Task Id</param>
        /// <returns>Next Task ID to be Executed</returns>
        private int Execute(int taskId)
        {
            var currentTask = TaskList.Where(t => t.TaskInfo.Order == taskId).ToList<TaskDictionary<int>>();
            foreach (var item in currentTask)
            {
                LogManager.WriteLog("Running Task: Order - " + item.TaskInfo.Order + ", Name - " + item.TaskInfo.Name, LogManager.enumLogLevel.Info);
                 item.Task.Start();
            }

            Task.WaitAll(currentTask.Select(t => t.Task).ToArray<Task>());

            currentTask.ForEach((t) => LogManager.WriteLog(string.Format("Task {0} Completed with Status {1} and Return Code {2}", t.TaskInfo.Name, t.Task.Status.ToString(),t.Task.Result.ToString()), LogManager.enumLogLevel.Info));

            if (currentTask[0].Task.Result >= 0)
            {
                LogManager.WriteLog("Task is a Success: Next Task Id - " + currentTask[0].TaskInfo.OnSucess, LogManager.enumLogLevel.Debug);
                currentTask.ForEach(t => t.Status = TaskStatus.Completed);

                if (int.Parse(currentTask[0].TaskInfo.OnSucess) == -1)
                    return 1;
                return Execute(int.Parse(currentTask[0].TaskInfo.OnSucess));
            }
            else
            {
                LogManager.WriteLog("Task is a Failure: Next Task Id - " + currentTask[0].TaskInfo.OnFailure, LogManager.enumLogLevel.Debug);
                currentTask.ForEach(t => t.Status = TaskStatus.Failed);

                if (int.Parse(currentTask[0].TaskInfo.OnFailure) == -1)
                    return -1;
                return Execute(int.Parse(currentTask[0].TaskInfo.OnFailure));
            }
        }

        /// <summary>
        /// Executes All Task based on given Order
        /// </summary>
        public void ExecuteTask()
        {
            foreach (var info in XmlHelper.TaskList.TaskInfo)
            {
                TaskList.Add(new TaskDictionary<int>(info, new Task<int>(() => BuildTaskFactory.GetBuildTask(info).Execute(info),TaskCreationOptions.LongRunning)));
            }
            Execute(TaskList.FirstOrDefault().TaskInfo.Order);
        }


    }
}
