using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Maersk.Sorting.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MaerskSortTest.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    [Route("sort")]
    public class SortController : ControllerBase
    {
        private readonly ISortJobProcessor _sortJobProcessor;
        private static Queue my_queue = new Queue();
        public SortController(ISortJobProcessor sortJobProcessorr)
        {
            _sortJobProcessor = sortJobProcessorr;
        }


        /// <summary>
        /// Enqueue Job: An endpoint to which clients can post a list of numbers to be sorted. The endpoint returns immediately, without requiring clients to wait for the jobs to complete
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<SortJob>> EnqueueJob(int[] values)
        {
            var pendingJob = new SortJob(
                          id: Guid.NewGuid(),
                          status: SortJobStatus.Pending,
                          duration: null,
                          input: values,
                          output: null);

            var completedJob = await _sortJobProcessor.Process(pendingJob);
            my_queue.Enqueue(completedJob);
            return pendingJob;
        }

        /// <summary>
        /// Return the current state of all jobs (both pending and completed).
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<SortJob>> GetJobs()
        {
            List<SortJob> sortJobs = new List<SortJob>();
            if (my_queue.Count == 0)
                return sortJobs;
            else
            {
                foreach(var sss in my_queue)
                {
                    var tt = sss as SortJob;
                    sortJobs.Add(tt);
                }
                return sortJobs;
            }
        }

        /// <summary>
        /// Returns the current state of a specific job.
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet("{jobId}")]
        public async Task<ActionResult<SortJob>> GetJob(string jobId)
        {
            
            foreach (var sss in my_queue)
            {
                var tt = sss as SortJob;
                if (tt.Id.ToString().Trim() == jobId.ToString().Trim())
                    return tt;             
            }
            return null;

        }

        #region Testing Api
        //[HttpPost("GetJob")]
        //public async Task<ActionResult<SortJob>> GetJob(int[] values)
        //{
        //    var pendingJob = new SortJob(
        //                   id: Guid.NewGuid(),
        //                   status: SortJobStatus.Pending,
        //                   duration: null,
        //                   input: values,
        //                   output: null);

        //    var completedJob = await _sortJobProcessor.Process(pendingJob);
        //    my_queue.Enqueue(completedJob);
        //    return pendingJob;
        //}

        //[HttpGet("GetJobById")]
        //public async Task<ActionResult<SortJob>> GetJobById(string id)
        //{
        //    var ss = my_queue.Peek() as SortJob;
        //    return ss;
        //}

        //[HttpGet("GetJobs")]
        //public async Task<List<SortJob>> GetJobs()
        //{
        //    List<SortJob> sortJobs = new List<SortJob>();
        //    if (my_queue.Count == 0)
        //        return sortJobs;
        //    else
        //    {
        //        for (int i = 0; i < my_queue.Count; i++)
        //        {
        //            var ss = my_queue.Peek() as SortJob;
        //            sortJobs.Add(ss);
        //        }
        //        return sortJobs;
        //    }

        //}
        #endregion
    }

}
