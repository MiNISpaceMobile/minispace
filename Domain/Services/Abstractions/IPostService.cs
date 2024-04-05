using Domain.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services;

public interface IPostService
{
    public Post CreatePost(Student author, Event @event, string content);
}
