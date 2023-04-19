using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;
using ToDoTask.Constants;
using ToDoTask.Data;
using ToDoTask.Data.Entities;
using ToDoTask.Models;
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
        public async Task<ActionResult> Index(string? searchString, int page)
        {
            List<Job> jobs;
            var projects = await _context.Projects.ToListAsync();
            ViewBag.Project = projects;
            var user = (from d in _context.ProjectUsers
                        join u in _context.Users on d.UserId equals u.Id
                        select new UserVm()
                        {
                            UserId = u.Id,
                            NameUser = u.Name
                        }).Distinct().ToList();
            ViewBag.Users = user;
            const int pageSize = 2;
            int totalJob = await _context.Jobs.CountAsync();
            if(totalJob > 0)
            {
                int countPage = (int)Math.Ceiling((double)totalJob / pageSize);
                if (page < 1)
                    page = 1;
                if (page > countPage)
                    page = countPage;
                if (!String.IsNullOrEmpty(searchString))
                {
                    var query = (from job in _context.Jobs
                                .Where(n => n.Name.Contains(searchString) || n.Content.Contains(searchString))
                                 orderby job.DateLine descending
                                 select job).Skip((page - 1) * pageSize).Take(pageSize);
                    jobs = await query.ToListAsync();
                }
                else
                {
                    var query = (from job in _context.Jobs
                                 orderby job.DateLine descending
                                 select job).Skip((page - 1) * pageSize).Take(pageSize);
                    jobs = await query.ToListAsync();
                }
                ViewData["CurrentFilter"] = searchString;
                ViewBag.CountPage = countPage;
                ViewBag.CurrentPage = page;

                return View(jobs);
            }
            else
            {
                return RedirectToAction("Create","Job");
            }
            
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
                return Content("Job is not found");
            _context.Jobs.Remove(job);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return RedirectToAction("Index", "Job");
            }
            return RedirectToAction("Index", "Job");
        }
        
        public async Task<bool> UpdateJob(int id, int status)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return false;
            if(status != job.Status)
            {
                if(status != 0)
                {
                    if(status == 2)
                    {
                        job.DateComplete = DateTime.Now;
                    }
                    
                    if (status == 1)
                    {
                        job.DateStart = DateTime.Now;
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
                if (result > 0) { return true; } else return false;
            }
            return true;
        }
        
        public async Task<IActionResult> ListJobWaitting(string? searchString, int page = 1)
        {
            List<JobForView> jobs;
            const int pageSize = 2;
            ViewData["CurrentFilter"] = searchString;
            int totalJob = await _context.Jobs.Where(j => j.Status == (int)Status.Waitting).CountAsync();
            if (totalJob > 0)
            {
                int countPage = (int)Math.Ceiling((double)totalJob / pageSize);
                jobs = await PagingForJob(_context, searchString, (int)Status.Waitting, page, pageSize, countPage);
                ViewBag.CountPage = countPage;
                ViewBag.CurrentPage = page;
                return View(jobs);
            }

            else
            {
                ViewBag.CountPage = 0;
                ViewBag.CurrentPage = page;
                return View();
            }
        }
        public async Task<IActionResult> ListJobInProgress(string? searchString, int page=1)
        {
            List<JobForView> jobs;
            const int pageSize = 2;
            ViewData["CurrentFilter"] = searchString;
            int totalJob = await _context.Jobs.Where(j => j.Status == (int)Status.Processing).CountAsync();
            if (totalJob > 0)
            {
                int countPage = (int)Math.Ceiling((double)totalJob / pageSize);
                jobs = await PagingForJob(_context, searchString, (int)Status.Processing, page, pageSize, countPage);
                ViewBag.CountPage = countPage;
                ViewBag.CurrentPage = page;
                return View(jobs);
            }

            else
            {
                ViewBag.CountPage = 0;
                ViewBag.CurrentPage = page;
                return View();
            }
        }
        public async Task<IActionResult> ListJobComplete(string? searchString, int page=1)
        {
            List<JobForView> jobs;
            const int pageSize = 2;
            ViewData["CurrentFilter"] = searchString;
            int totalJob = await _context.Jobs.Where(j => j.Status == (int)Status.Done).CountAsync();
            if (totalJob > 0)
            {
                int countPage = (int)Math.Ceiling((double)totalJob / pageSize);
                jobs = await PagingForJob(_context, searchString, (int)Status.Done, page, pageSize, countPage);
                ViewBag.CountPage = countPage;
                ViewBag.CurrentPage = page;
                return View(jobs);
            }

            else
            {
                ViewBag.CountPage = 0;
                ViewBag.CurrentPage = page;
                return View();
            }
        }
        public static async Task<List<JobForView>> PagingForJob(ApplicationDbContext context,string? searchString,int statusJob,int page, int pageSize,int countPage)
        {
            List<JobForView> jobs;
            if(page < 1) { page = 1; }
            if(page > countPage) { page = countPage; }
            if (!String.IsNullOrEmpty(searchString))
            {
                var query = (from j in context.Jobs
                             join p in context.Projects on j.ProjectId equals p.Id
                             join u in context.Users on j.UserId equals u.Id
                             where ((j.Status == statusJob) && (j.Name.Contains(searchString) || j.Content.Contains(searchString)))
                             select new JobForView()
                             {
                                 ProjectName = p.Name,
                                 Id = j.Id,
                                 Name = j.Name,
                                 Content = j.Content,
                                 DateLine = j.DateLine,
                                 UserName = u.Name,
                                 DateStart = j.DateStart,
                                 DateAssign = j.DateAssign,
                                 Status = j.Status,
                                 DateComplete = j.DateComplete
                             }).Skip((page - 1) * pageSize).Take(pageSize);

                jobs = await query.ToListAsync();
            }
            else
            {
                var query = (from j in context.Jobs
                             join p in context.Projects on j.ProjectId equals p.Id
                             join u in context.Users on j.UserId equals u.Id
                             where j.Status == statusJob
                             select new JobForView()
                             {
                                 ProjectName = p.Name,
                                 Id = j.Id,
                                 Name = j.Name,
                                 Content = j.Content,
                                 DateLine = j.DateLine,
                                 UserName = u.Name,
                                 DateStart = j.DateStart,
                                 DateAssign = j.DateAssign,
                                 Status = j.Status,
                                 DateComplete=j.DateComplete
                             }).Skip((page - 1) * pageSize).Take(pageSize);

                jobs = await query.ToListAsync();
            }
            
            return jobs;
        }

    }
}
