namespace Dapper.API.Dtos.Common
{
    /// <summary>
    /// Base model for dtos
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// Numeric Id of the entity/model
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The status of the entity
        /// 1 = Active, 2 = Inactive, 3 = DeletedForEveryone, 4 = Pending, 5 = Archived, 6 = Suspended, 7 = DeletedForMe
        /// </summary>
        public int EntityStatusId { get; set; }

        /// <summary>
        /// The datetime the record was created
        /// </summary>
        public DateTime? CreateAt { get; set; }
    }
}
