using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;
using ToDoTask.Constants;
using ToDoTask.Data;
using ToDoTask.Data.Entities;
using ToDoTask.Models.Contents;
using ToDoTask.Services;
using static System.Reflection.Metadata.BlobBuilder;

namespace ToDoTask.Controllers

{
    [Authorize]
    public class JobController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISequenceService _sequenceService;
        public JobController(ApplicationDbContext context, ISequenceService sequenceService)
        {
            _context = context;
            _sequenceService = sequenceService;
        }
        // GET: JobController
        public async Task<ActionResult> Index()
        {
            var Job = await _context.Jobs.ToListAsync();
            var project = await _context.Projects.ToListAsync();
            ViewBag.Project = project;
            return View(Job);
        }

        // GET: JobController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        [Authorize]
        // GET: JobController/Create
        public async Task<ActionResult> Create()
        {
            var project = (from d in _context.Projects
                           join f in _context.ProjectUsers on d.Id equals f.ProjectId
                           select new
                           {
                               ProjectId = d.Id,
                               ProjectName = d.Name
                           }).Distinct().ToList();
            ViewBag.Project = project;
            return View();
        }
        //POST : Job/loadUserOfProject
        [HttpPost]
        public async Task<List<User>> loadUserOfProject(string projectId)
        {
            var user = (from d in _context.ProjectUsers
                        join f in _context.Users on d.UserId equals f.Id
                        where d.ProjectId == projectId
                        select new User()
                        {
                            Id = f.Id,
                            Name = f.Name
                        }).ToList();
            return user;
        }

        // POST: JobController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Job jobRequest)
        {
            try
            {               
                var job = new Job()
                {
                    Status = (int) Status.Waitting,
                    Name = jobRequest.Name,
                    ProjectId = jobRequest.ProjectId,
                    Content = jobRequest.Content,
                    DateLine = jobRequest.DateLine,
                    UserId = jobRequest.UserId,
                    DateAssign = DateTime.Now
                };
                var JobId = await _sequenceService.GetNewId();
                job.Id = JobId;
                _context.Jobs.Add(job);
                _context.SaveChanges();
                return RedirectToAction("Index", "Job");
            }
            catch
            {
                return View();
            }
        }
        // GET: JobController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var project = (from d in _context.Projects
                               join f in _context.ProjectUsers on d.Id equals f.ProjectId
                               select new
                               {
                                   ProjectId = d.Id,
                                   ProjectName = d.Name
                               }).Distinct().ToList();
            ViewBag.Project = project;
            ViewBag.Project = project;
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return RedirectToAction("Index", "Job");
            return View(job);
        }

        // POST: JobController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: JobController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return NotFound("Job is not found");
            _context.Jobs.Remove(job);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return RedirectToAction("Index", "Job");
            }
            return RedirectToAction("Index", "Job");
        }
        
        public async Task<String> UpdateJob(int id, int status)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return "Job is not found";
            if(status != job.Status)
            {
                if(status != 0)
                {
                    if(status == 2)
                    {
                        job.DateComplete = DateTime.Now;
                    }

                    job.DateStart = DateTime.Now;
                    if (status == 1)
                    {
                        var project = await _context.Projects.FindAsync(job.ProjectId);
                        if (project != null)
                        {
                            project.Status = (int)Status.Processing;
                            _context.Projects.Update(project);
                        }
                    }
                }
                job.Status = status;
                _context.Jobs.Update(job);
                var result =  await _context.SaveChangesAsync();
                if (result > 0) { return "OK"; } else return "Not OK";
            }
            return "OK";
        }
        
        public async Task<IActionResult> ListJobWaitting()
        {
            var job = (from j in _context.Jobs
                       join p in _context.Projects on j.ProjectId equals p.Id
                       join u in _context.Users on j.UserId equals u.Id
                       where j.Status == (int)Status.Waitting
                       select new JobForView()
                           {
                               ProjectName = p.Name,
                               Id = j.Id,
                               Name = j.Name,
                               Content = j.Content,
                               DateLine = j.DateLine,
                               UserName  = u.Name,
                               DateAssign = j.DateAssign,
                               Status = (int)Status.Waitting,
                           }).Distinct().ToList();
            return View(job);
        }
        public async Task<IActionResult> ListJobInProgress()
        {
            var job = (from j in _context.Jobs
                       join p in _context.Projects on j.ProjectId equals p.Id
                       join u in _context.Users on j.UserId equals u.Id
                       where j.Status == (int)Status.Processing
                       select new JobForView()
                       {
                            ProjectName = p.Name,
                            Id = j.Id,
                            Name = j.Name,
                            Content = j.Content,
                            Status = j.Status,
                            DateStart = j.DateStart,
                            UserName  = u.Name,
                            UserId = u.Id
                       }).Distinct().ToList();
            return View(job);
        }
        public async Task<IActionResult> ListJobComplete()
        {
            var job = (from j in _context.Jobs
                       join p in _context.Projects on j.ProjectId equals p.Id
                       join u in _context.Users on j.UserId equals u.Id
                       where j.Status == (int)Status.Done
                       select new JobForView()
                           {
                               ProjectName = p.Name,
                               Id = j.Id,
                               Name = j.Name,
                               Content = j.Content,
                               Status = j.Status,
                               DateStart = j.DateStart,
                               DateComplete = j.DateComplete,
                               UserName  = u.Name
                           }).Distinct().ToList();
            return View(job);
        }

    }
}
