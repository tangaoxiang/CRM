/****** Script for SelectTopNRows command from SSMS  ******/
SELECT * from sysFunction where mID=4
select * from sysMenus
select * from dbo.sysPermissList where rID=12
--1.0 权限
select * from sysUserInfo where uLoginName='jishujingli' and uLoginPWD='E10ADC3949BA59ABBE56E057F20F883E'
--2.0 获取当前用户所拥有的角色
select * from sysUserInfo_Role where uID=11

--3.0 查询出当前角色拥有的权限菜单
select * from sysMenus where mID in(
select mID from dbo.sysPermissList where rID=12
)

--使用in操作来实现权限菜单获取,性能不好
select * from sysMenus where mID in
(
select mID from dbo.sysPermissList where rID  in 
(
  select rID from sysUserInfo_Role where uID=11
)
)

--使用join
select distinct m.* from sysMenus m 
inner join sysPermissList p on (m.mID=p.mID)
inner join sysUserInfo_Role ur on (p.rID=ur.rID and ur.uID=11)