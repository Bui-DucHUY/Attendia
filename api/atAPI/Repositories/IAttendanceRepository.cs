using Attendia.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendia.Repositories
{
    public interface IAttendanceRepository
    {
        Task<bool> CheckInAsync(AttendanceRecord record);
        Task<IEnumerable<AttendanceRecord>> GetRecordsBySessionAsync(Guid sessionId);
        Task<bool> ApproveRecordAsync(Guid recordId, bool isApproved); 
        Task<bool> DeleteRecordAsync(Guid recordId);
    }
}