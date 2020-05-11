CREATE PROCEDURE PromoteStudents
    @Studies NVARCHAR(10),
    @Semester INT  
AS
BEGIN
	SET XACT_ABORT ON;
	BEGIN TRANSACTION;

	DECLARE @IdStudies INT = (SELECT IdStudy FROM Studies WHERE Name=@Studies);
	IF @IdStudies IS NULL
	BEGIN
		ROLLBACK TRANSACTION;
		RETURN
	END

	DECLARE @IdEnrollemnt INT = (SELECT IdEnrollment FROM Enrollment WHERE IdStudy=@IdStudies AND Semester = @Semester + 1);
	IF @IdEnrollemnt IS NULL
	BEGIN
		INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy,	StartDate) VALUES((SELECT MAX(IdEnrollment) + 1 FROM Enrollment), @Semester + 1, @IdStudies, GETDATE());
	END
	
	DECLARE @OldIdEnrollemnt INT = (SELECT IdEnrollment FROM Enrollment WHERE IdStudy=@IdStudies AND Semester = @Semester);
	IF @OldIdEnrollemnt IS NULL
	BEGIN
		ROLLBACK TRANSACTION;
		RETURN;
	END

	UPDATE Student SET IdEnrollment = @OldIdEnrollemnt + 1 WHERE IdEnrollment = @OldIdEnrollemnt;

	COMMIT TRANSACTION;

END
