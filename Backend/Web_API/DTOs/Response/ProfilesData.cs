﻿namespace Web_API.DTOs;

public class ProfilesData
{
    public List<ProfileForOtherUsers> Profiles { get; set; }
    public int ProfilesCount { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}