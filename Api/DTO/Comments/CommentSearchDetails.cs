using Api.DTO.Paging;

namespace Api.DTO.Comments;

// APISpec says that only one id should be present
public record CommentSearchDetails(
    long? EventId,
    long? PostId,
    PageableRequest Pageable);
