USE [GOLF_WAREHOUSE]
GO

/****** Object:  StoredProcedure [dbo].[PS_DOC_HDR_Create]    Script Date: 6/16/2024 6:00:57 PM ******/
DROP PROCEDURE [dbo].[PS_DOC_HDR_Create]
GO

/****** Object:  StoredProcedure [dbo].[PS_DOC_HDR_Create]    Script Date: 6/16/2024 6:00:57 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[PS_DOC_HDR_Create]
	@DOC_ID T_DOC_ID,
	@STR_ID T_COD,
	@STA_ID T_COD,
	@TKT_NO T_DOC_NO,
	@DOC_TYP T_FLG,
	@CUST_NO T_CUST_NO,
	@LINS T_INT,
	@LIN_TOT T_MONEY,
	@PS_DOC_LIN NVARCHAR(MAX)
AS
BEGIN
    -- Ensure no additional result sets are sent to the client
    SET NOCOUNT ON;

    -- Begin transaction
    BEGIN TRY
        -- Begin the transaction
        BEGIN TRANSACTION;

        -- Add record
        INSERT INTO PS_DOC_HDR(
			DOC_ID, 
			STR_ID, 
			STA_ID, 
			TKT_NO, 
			DOC_TYP, 
			CUST_NO, 
			LINS, 
			LIN_TOT,
			DOC_GUID)
        VALUES(
			@DOC_ID, 
			@STR_ID, 
			@STA_ID, 
			@TKT_NO, 
			@DOC_TYP, 
			@CUST_NO, 
			@LINS, 
			@LIN_TOT,
			NEWID());

		-- Insert DetailS if JSON data is provided
		IF @PS_DOC_LIN IS NOT NULL
		BEGIN
			INSERT INTO PS_DOC_LIN (
				DOC_ID, 
				STR_ID, 
				STA_ID, 
				TKT_NO, 
				LIN_SEQ_NO,
				LIN_TYP,
				ITEM_NO,
				QTY_SOLD,
				QTY_NUMER,
				QTY_DENOM,
				SELL_UNIT,
				EXT_PRC,
				IS_TXBL,
				ITEM_TYP,
				TRK_METH,
				LIN_GUID,
				QTY_RET,
				GROSS_EXT_PRC,
				HAS_PRC_OVRD,
				USR_ENTD_PRC,
				IS_DISCNTBL,
				CALC_EXT_PRC,
				IS_WEIGHED,
				TAX_AMT_ALLOC,
				NORM_TAX_AMT_ALLOC)
			SELECT 				
				@DOC_ID, 
				@STR_ID, 
				@STA_ID, 
				@TKT_NO, 
				LIN_SEQ_NO,
				LIN_TYP,
				ITEM_NO,
				QTY_SOLD,
				QTY_NUMER,
				QTY_DENOM,
				SELL_UNIT,
				EXT_PRC,
				IS_TXBL,
				ITEM_TYP,
				TRK_METH,
				NEWID(),
				QTY_RET,
				GROSS_EXT_PRC,
				HAS_PRC_OVRD,
				USR_ENTD_PRC,
				IS_DISCNTBL,
				CALC_EXT_PRC,
				IS_WEIGHED,
				TAX_AMT_ALLOC,
				NORM_TAX_AMT_ALLOC
			FROM OPENJSON(@PS_DOC_LIN)
			WITH (
				LIN_SEQ_NO T_SEQ_NO '$.LIN_SEQ_NO',
				LIN_TYP T_FLG '$.LIN_TYP',
				ITEM_NO T_ITEM_NO '$.ITEM_NO',
				QTY_SOLD T_QTY '$.QTY_SOLD',
				QTY_NUMER T_CONV_FAC '$.QTY_NUMER',
				QTY_DENOM T_CONV_FAC '$.QTY_DENOM',
				SELL_UNIT T_FLG '$.SELL_UNIT',
				EXT_PRC T_MONEY '$.EXT_PRC',
				IS_TXBL T_BOOL '$.IS_TXBL',
				ITEM_TYP T_FLG '$.ITEM_TYP',
				TRK_METH T_FLG '$.TRK_METH',
				QTY_RET T_QTY '$.QTY_RET',
				GROSS_EXT_PRC T_MONEY '$.GROSS_EXT_PRC',
				HAS_PRC_OVRD T_BOOL '$.HAS_PRC_OVRD',
				USR_ENTD_PRC T_BOOL '$.USR_ENTD_PRC',
				IS_DISCNTBL T_BOOL '$.IS_DISCNTBL',
				CALC_EXT_PRC T_MONEY '$.CALC_EXT_PRC',
				IS_WEIGHED T_BOOL '$.IS_WEIGHED',
				TAX_AMT_ALLOC T_MONEY '$.TAX_AMT_ALLOC',
				NORM_TAX_AMT_ALLOC T_MONEY '$.NORM_TAX_AMT_ALLOC'
			) AS JSONData;
		END
		
        -- Commit the transaction
        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        -- Rollback the transaction if an error occurs
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END

        -- Raise an error or handle it as required
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;

    -- Ensure that no additional result sets are sent to the client
    SET NOCOUNT OFF;
END;
GO


