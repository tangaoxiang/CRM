
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<ivna>
-- Create date: <Create Date,,>
-- Description:	<�����û�id��ȡ���û���ϵͳ�е�����Ȩ�ް�ť����>
-- =============================================
CREATE PROCEDURE Usp_GetFunctionsForUser15
	@uid int  --��ǰ��¼���û�id
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  select f.fName,f.fFunction,f.fPicname,m.mUrl,m.mArea,m.mController,m.mAction from sysFunction f
join sysMenus m on (f.mID=m.mID)
join sysPermissList p on (f.fID=p.fID)
join sysUserInfo_Role ur on (p.rID=ur.rID and ur.uID=@uid) 
END
GO
