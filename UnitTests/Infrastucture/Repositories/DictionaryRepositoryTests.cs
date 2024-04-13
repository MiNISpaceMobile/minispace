using Domain.DataModel;
using Infrastructure.UnitOfWorks;

namespace UnitTests.Infrastucture.Repositories;

[TestClass]
public class DictionaryRepositoryTests
{
#pragma warning disable CS8618 // Unassigned non-nullables
    private Administrator ad;
    private Student st;

    private Event ev;
    private Post po;
    private Comment co;

    private EventReport er;
    private PostReport pr;
    private CommentReport cr;

    private DictionaryUnitOfWork sut;
#pragma warning restore CS8618 // Unassigned non-nullables

    [TestInitialize]
    public void PreTest()
    {
        DateTime now = DateTime.Now;

        ad = new Administrator("ad", "ad", "ad");
        st = new Student("st", "st", "st");

        ev = new Event(st, "ev", "ev", EventCategory.Uncategorized, now, now, now, "ev", null, null);
        po = new Post(st, ev, "po");
        co = new Comment(st, po, "co", null);

        er = new EventReport(ev, ad, "er", "er", ReportCategory.Bug);
        pr = new PostReport(po, ad, "pr", "pr", ReportCategory.Behaviour);
        cr = new CommentReport(co, ad, "cr", "cr", ReportCategory.Bug);

        sut = new DictionaryUnitOfWork([ad, st, ev, po, co, er, pr, cr]);
    }

    // TODO: Add tests
}
