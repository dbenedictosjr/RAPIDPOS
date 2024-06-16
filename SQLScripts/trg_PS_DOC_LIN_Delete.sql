USE [GOLF_WAREHOUSE]
GO

/****** Object:  Trigger [dbo].[trg_PS_DOC_LIN_Delete]    Script Date: 6/16/2024 7:00:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TRIGGER [dbo].[trg_PS_DOC_LIN_Delete]
ON [dbo].[PS_DOC_LIN]
AFTER DELETE
AS
BEGIN
	DELETE	USER_SUGGESTED_REPLENISHMENT
	WHERE	DOC_ID IN(SELECT DOC_ID FROM deleted)
	AND		ITEM_NO IN(SELECT ITEM_NO FROM deleted)
END;
GO

ALTER TABLE [dbo].[PS_DOC_LIN] ENABLE TRIGGER [trg_PS_DOC_LIN_Delete]
GO


