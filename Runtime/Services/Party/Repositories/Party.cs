using System.Threading.Tasks;
using DynamicPixels.GameService.Models.outputs;
using DynamicPixels.GameService.Services.Party.Models;
using DynamicPixels.GameService.Utils.HttpClient;

namespace DynamicPixels.GameService.Services.Party.Repositories
{
    public class PartyRepository : IPartyRepository
    {
        public Task<RowListResponse<Party>> GetParties<T>(T input) where T : GetPartiesParams
        {
            return WebRequest.Get<RowListResponse<Party>>(UrlMap.GetPartiesUrl);
        }

        public Task<RowResponse<Party>> CreateParty<T>(T input) where T : CreatePartyParams
        {
            return WebRequest.Post<RowResponse<Party>>(UrlMap.CreatePartyUrl, input.ToString());
        }

        public Task<RowListResponse<Party>> GetSubscribedParties<T>(T input) where T : GetSubscribedPartiesParams
        {
            return WebRequest.Get<RowListResponse<Party>>(UrlMap.GetSubscribedPartiesUrl);
        }

        public Task<RowResponse<Party>> GetPartyById<T>(T input) where T : GetPartyByIdParams
        {
            return WebRequest.Get<RowResponse<Party>>(UrlMap.GetPartyByIdUrl(input.PartyId));
        }

        public Task<RowResponse<PartyMember>> JoinToParty<T>(T input) where T : JoinToPartyParams
        {
            return WebRequest.Post<RowResponse<PartyMember>>(UrlMap.JoinToPartyUrl(input.PartyId), input.ToString());
        }

        public Task<RowResponse<Party>> EditParty<T>(T input) where T : EditPartyParams
        {
            return WebRequest.Put<RowResponse<Party>>(UrlMap.EditPartyUrl(input.PartyId), input.ToString());
        }

        public Task<ActionResponse> LeaveParty<T>(T input) where T : LeavePartyParams
        {
            return WebRequest.Delete<ActionResponse>(UrlMap.LeavePartyUrl(input.PartyId));
        }

        public Task<RowListResponse<RichPartyMember>> GetPartyMembers<T>(T input) where T : GetPartyMembersParams
        {
            return WebRequest.Get<RowListResponse<RichPartyMember>>(UrlMap.GetPartyMembersUrl(input.PartyId));
        }

        public Task<RowListResponse<PartyMember>> GetPartyWaitingMembers<T>(T input) where T : GetPartyWaitingMembersParams
        {
            return WebRequest.Get<RowListResponse<PartyMember>>(UrlMap.GetPartyWaitingMembersUrl(input.PartyId));
        }

        public Task<RowResponse<PartyMember>> AcceptJoining<T>(T input) where T : AcceptJoiningParams
        {
            return WebRequest.Put<RowResponse<PartyMember>>(UrlMap.AcceptJoiningUrl(input.PartyId, input.MembershipId));
        }

        public Task<RowResponse<PartyMember>> RejectJoining<T>(T input) where T : RejectJoiningParams
        {
            return WebRequest.Delete<RowResponse<PartyMember>>(UrlMap.RejectJoiningUrl(input.PartyId, input.MembershipId));
        }
    }
}