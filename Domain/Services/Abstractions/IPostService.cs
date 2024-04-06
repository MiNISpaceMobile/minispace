using Domain.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services;

public interface IPostService
{
    public Post CreatePost(Guid authorGuid, Guid eventGuid, string content);
    public Post GetPost(Guid guid);
    public void DeletePost(Guid guid);
}
