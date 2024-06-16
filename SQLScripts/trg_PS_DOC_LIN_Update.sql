USE [GOLF_WAREHOUSE]
GO

/****** Object:  Trigger [dbo].[trg_PS_DOC_LIN_Update]    Script Date: 6/16/2024 6:59:45 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TRIGGER [dbo].[trg_PS_DOC_LIN_Update]
ON [dbo].[PS_DOC_LIN]
AFTER UPDATE
AS
BEGIN
	UPDATE	suggested
	SET		suggested.QTY = inv.MAX_QTY-(inv.QTY_AVAIL - ins.QTY_SOLD) -- suggested qty
	FROM	USER_SUGGESTED_REPLENISHMENT AS suggested
	INNER JOIN inserted AS ins
		ON ins.DOC_ID = suggested.DOC_ID AND ins.LIN_SEQ_NO = suggested.ITEM_NO
	INNER JOIN IM_INV AS inv
		ON inv.ITEM_NO = ins.ITEM_NO
END;
GO

ALTER TABLE [dbo].[PS_DOC_LIN] ENABLE TRIGGER [trg_PS_DOC_LIN_Update]
GO


