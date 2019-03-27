
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<ivan>
-- Create date: <2014-9-22>
-- Description:	<根据用户id获取其权限菜单数据>
-- =============================================
CREATE PROCEDURE Usp_GetPermissMenusForUser
	@uid int --当前用户的id
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  select distinct m.* from sysMenus m 
		inner join sysPermissList p on (m.mID=p.mID)
		inner join sysUserInfo_Role ur on (p.rID=ur.rID and ur.uID=@uid)
		END
GO
