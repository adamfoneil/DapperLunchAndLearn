using Dapper;
using DapperLunchAndLearn;
using Dommel;
using DapperLunchAndLearn.Dommel;
using System.Linq;

using var cn = Connection.Open();

var objId = await cn.InsertAsync(new Artist()
{
    Name = "Beck",
    CreatedBy = "adamo"
});

var id = Convert.ToInt32(objId);