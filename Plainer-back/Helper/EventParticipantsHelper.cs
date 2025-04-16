using System;
using Plainer.Data;
using Plainer.Entities;

namespace Plainer.Helper;

public static class EventParticipantsHelper
{

        public static bool EventParticipantChecks(EventParticipant? currentParticipant, EventParticipant? targetParticipant){
            if (currentParticipant is null || targetParticipant is null)
            {
                return false;
            }
            if (currentParticipant.RoleId >= targetParticipant.RoleId)
            {
                    return false;
            }
            return true;
        }

}
