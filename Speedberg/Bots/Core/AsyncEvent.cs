using System.Threading.Tasks;

namespace Speedberg.Bots.Core
{
    public delegate Task AsyncEvent();
    public delegate Task AsyncEvent<in T>(T obj);
}