﻿namespace Domain.BaseTypes;

// Every class that has its instances stored in database should inherit this class
public abstract class BaseEntity
{
    public Guid Guid { get; set; }
}

