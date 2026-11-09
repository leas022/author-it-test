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
        [EndpointName("getTopics")]
        [ProducesResponseType(typeof(IReadOnlyList<TopicResponse>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyList<TopicResponse>> GetAll()
        {
            return Ok(_topicsService.GetAll());
        }

        [HttpGet("{id}")]
        [EndpointName("getTopicById")]
        [ProducesResponseType(typeof(TopicResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<TopicResponse> GetById(int id)
        {
            var topic = _topicsService.GetById(id);
            if (topic == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound,
                    title: "Topic not found", detail: "The requested topic does not exist.");
            }
            return Ok(topic);
        }

        [HttpPost]
        [EndpointName("createTopic")]
        [ProducesResponseType(typeof(TopicResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<TopicResponse> Create([FromBody] CreateTopicRequest request)
        {
            var topic = _topicsService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = topic.Id }, topic);
        }

        [HttpPut("{id}")]
        [EndpointName("updateTopic")]
        [ProducesResponseType(typeof(TopicResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<TopicResponse> Update(int id, [FromBody] UpdateTopicRequest request)
        {
            var updateResponse = _topicsService.Update(id, request);
            if (!updateResponse.TopicExists)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound,
                    title: "Topic not found", detail: "The requested topic does not exist.");
            }

            if (updateResponse.Topic == null)
            {
                return Problem(statusCode: StatusCodes.Status409Conflict,
                    title: "Topic update conflict",
                    detail: "The topic has been modified by another user.");
            }

            return Ok(updateResponse.Topic);
        }

        [HttpGet("{id}/usage")]
        [EndpointName("getTopicUsage")]
        [ProducesResponseType(typeof(TopicUsageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<TopicUsageResponse> GetTopicUsage(int id)
        {
            var usage = _topicsService.GetTopicUsage(id);
            if (usage == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound,
                    title: "Topic not found", detail: "The requested topic does not exist.");
            }
            return Ok(usage);
        }
    }
}
