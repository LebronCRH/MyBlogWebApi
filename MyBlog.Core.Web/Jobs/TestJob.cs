using MyBlog.Core.IServices;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Core.Web.Jobs
{
    public class TestJob : IJob
    {
        private readonly IBlogArticleServices _blogArticleServices;
        public TestJob(IBlogArticleServices blogArticleServices)
        {
            _blogArticleServices = blogArticleServices;
        }


        public async Task Execute(IJobExecutionContext context)
        {
            var article = await _blogArticleServices.QueryById(1);
            article.ArticleVisitNumber += 1;
            await _blogArticleServices.Update(article);
            Trace.WriteLine("定时任务 Test  " + DateTime.Now.ToString());
        }
    }

    public class TestTriggerServer
    {

        public ITrigger TestTrigger()
        {
            var trigger = TriggerBuilder.Create()
                .WithIdentity("MyBlogTrigger", "name")
               .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)
                    .WithRepeatCount(10)) // note that 10 repeats will give a total of 11 firings
                .Build();
            return trigger;
        }
    }
}
