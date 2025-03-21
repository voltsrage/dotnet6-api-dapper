namespace Dapper.API.Entities.Common
{
    /// <summary>
    /// Contains common properties for all entities
    /// </summary>
    public class BaseEntity
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
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// The person who created the record
        /// </summary>

        public int CreatedBy { get; set; }


        /// <summary>
        /// The datetime the record was updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// The person who last updated the record
        /// </summary>

        public int UpdatedBy { get; set; }
    }
}
