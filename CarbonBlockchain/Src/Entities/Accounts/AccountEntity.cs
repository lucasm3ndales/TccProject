using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarbonBlockchain.Entities.Accounts;

public class AccountEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string OwnerName { get; set; }
    public string OwnerDocument { get; set; }
    public string AccountAddress { get; set; }
    public string AccountPrivateKey { get; set; }
}