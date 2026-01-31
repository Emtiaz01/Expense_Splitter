namespace ExpenseSplitter.DTOs
{
    public class BalanceDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal TotalPaid { get; set; }
        public decimal TotalShare { get; set; }
        public decimal Balance { get; set; }
    }

    public class SettlementDto
    {
        public int SettlementId { get; set; }
        public int FromUserId { get; set; }
        public string FromUserName { get; set; } = string.Empty;
        public int ToUserId { get; set; }
        public string ToUserName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Note { get; set; }
    }

    public class CreateSettlementDto
    {
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public decimal Amount { get; set; }
        public string? Note { get; set; }
    }
}
