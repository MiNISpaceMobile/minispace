using Api.DTO.Paging;

namespace Api.DTO.Users;

public record UserSearchDetails(
    string Name,
    long StudentNumber,
    PageableRequest Pageable);