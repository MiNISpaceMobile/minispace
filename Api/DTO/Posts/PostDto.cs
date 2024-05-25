﻿using Api.DTO.Users;

namespace Api.DTO.Posts;

public record PostDto(
    Guid Guid,
    Guid EventGuid,
    PublicUserDto? Author,
    DateTime CreationDate);
