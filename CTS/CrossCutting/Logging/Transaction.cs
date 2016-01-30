using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossCutting.Logging
{
    public class Transaction
    {
        public Transaction()
        {
            TransactionDetail = new List<TransactionDetail>();
        }
        public Guid TransactionId { get; set; }
        public string TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime TransactionDateUtc { get; set; }
        public string ModifiedBy { get; set; }

        public List<TransactionDetail> TransactionDetail { get; set; }

        public void AddDetail(string tableName, string crudOperation, string transactionType)
        {
            if (TransactionDetail.FirstOrDefault(t => t.TableName == tableName) == null)
            {
                TransactionDetail.Add(
                    new TransactionDetail
                    {
                        TransactionId = TransactionId,
                        TableName = tableName,
                        CrudOperation = crudOperation,
                        TransactionType = transactionType
                    });
            }
        }
    }
}

