# Unit Testing Check List

16 integration tests (xUnit + WebApplicationFactory + isolated MongoDB) — `app/src/HeritageTrail.Tests/SpecTests.cs`.
Run: `dotnet test` (in `app/`). Result: **16/16 passed**.

| BA case | Test |
|---|---|
| Register ok / duplicate | Register_ValidUser_RedirectsToAccount · Register_DuplicateEmail_IsRejected |
| Guest → admin | Guest_AdminPage_RedirectsToLogin |
| User → admin 403 | NonAdminUser_AdminPage_IsDenied |
| Search empty / category | Search_NoResults_ShowsEmptyMessage · Search_Category_FiltersSites |
| Admin adds site | Admin_AddsSite_AppearsOnPublicList |
| Category delete blocked | Admin_DeletesCategoryInUse_IsBlocked |
| Interest duplicate / past event | Interest_Toggle_NeverDuplicates · Interest_PastEvent_IsRejected |
| Invite invalid email | Invite_InvalidEmail_IsNotRecorded |
| Query replied | Query_AdminReply_IsVisibleToUser |
| End < Start | Event_EndBeforeStart_IsRejected |
| .exe upload rejected / png accepted | Upload_Exe_IsRejected · Upload_Png_IsAcceptedAndServed |
| No password exposure | MembersPage_DoesNotExposePasswordHashes |
