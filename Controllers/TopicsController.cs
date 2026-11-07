using AuthorIt.Dtos;
using AuthorIt.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthorIt.Controllers
{
    [Route("api/topics")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly TopicsService _topicsService;
        public TopicsController(TopicsService topicsService)
        {
            _topicsService = topicsService;
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<TopicResponse>> GetAll()
        {
            return Ok(_topicsService.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<TopicResponse> GetById(int id)
        {
            var topic = _topicsService.GetById(id);
            if (topic == null)
            {
                return NotFound();
            }
            return Ok(topic);
        }

        [HttpPost]
        public ActionResult<TopicResponse> Create([FromBody] CreateTopicRequest request)
        {
            var topic = _topicsService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = topic.Id }, topic);
        }

        [HttpPut("{id}")]
        public ActionResult<TopicResponse> Update(int id, [FromBody] UpdateTopicRequest request)
        {
            var updateResponse = _topicsService.Update(id, request);
            if (!updateResponse.TopicExists)
            {
                return NotFound();
            }

            if (updateResponse.Topic == null)
            {
                return Conflict("The topic has been modified by another user. Please refresh and try again.");
            }

            return Ok(updateResponse.Topic);
        }
    }
}
