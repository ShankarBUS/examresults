namespace ExamResultsCLI;

/// <summary>
/// Represents a student's result entry with detailed marks and results for various subjects.
/// </summary>
public class StudentResultEntry
{
    /// <summary>
    /// Gets or sets the registration number of the student.
    /// </summary>
    public string? RegNo { get; set; }

    /// <summary>
    /// Gets or sets the name of the student.
    /// </summary>
    public string? Name { get; set; }

    #region General Medicine (IM)

    /// <summary>
    /// Gets or sets the marks for General Medicine Theory Paper I.
    /// </summary>
    public string? IMTheoryI { get; set; }

    /// <summary>
    /// Gets or sets the marks for General Medicine Theory Paper II.
    /// </summary>
    public string? IMTheoryII { get; set; }

    /// <summary>
    /// Gets or sets the total marks for General Medicine Theory.
    /// </summary>
    public string? IMTheoryTotal { get; set; }

    /// <summary>
    /// Gets or sets the marks for General Medicine Practical.
    /// </summary>
    public string? IMPrac { get; set; }

    /// <summary>
    /// Gets or sets the total marks for General Medicine.
    /// </summary>
    public string? IMTotal { get; set; }

    /// <summary>
    /// Gets or sets the result for General Medicine.
    /// </summary>
    public string? IMResult { get; set; }

    #endregion

    #region General Surgery (SU)

    /// <summary>
    /// Gets or sets the marks for General Surgery Theory Paper I.
    /// </summary>
    public string? SUTheoryI { get; set; }

    /// <summary>
    /// Gets or sets the marks for General Surgery Theory Paper II.
    /// </summary>
    public string? SUTheoryII { get; set; }

    /// <summary>
    /// Gets or sets the total marks for General Surgery Theory.
    /// </summary>
    public string? SUTheoryTotal { get; set; }

    /// <summary>
    /// Gets or sets the marks for General Surgery Practical.
    /// </summary>
    public string? SUPrac { get; set; }

    /// <summary>
    /// Gets or sets the total marks for General Surgery.
    /// </summary>
    public string? SUTotal { get; set; }

    /// <summary>
    /// Gets or sets the result for General Surgery.
    /// </summary>
    public string? SUResult { get; set; }

    #endregion

    #region Obstetrics and Gynaecology (OG)

    /// <summary>
    /// Gets or sets the marks for Obstetrics and Gynaecology Theory Paper I.
    /// </summary>
    public string? OGTheoryI { get; set; }

    /// <summary>
    /// Gets or sets the marks for Obstetrics and Gynaecology Theory Paper II.
    /// </summary>
    public string? OGTheoryII { get; set; }

    /// <summary>
    /// Gets or sets the total marks for Obstetrics and Gynaecology Theory.
    /// </summary>
    public string? OGTheoryTotal { get; set; }

    /// <summary>
    /// Gets or sets the marks for Obstetrics and Gynaecology Practical.
    /// </summary>
    public string? OGPrac { get; set; }

    /// <summary>
    /// Gets or sets the total marks for Obstetrics and Gynaecology.
    /// </summary>
    public string? OGTotal { get; set; }

    /// <summary>
    /// Gets or sets the result for Obstetrics and Gynaecology.
    /// </summary>
    public string? OGResult { get; set; }

    #endregion

    #region Pediatrics (PE)

    /// <summary>
    /// Gets or sets the marks for Pediatrics Theory.
    /// </summary>
    public string? PETheory { get; set; }

    /// <summary>
    /// Gets or sets the marks for Pediatrics Practical.
    /// </summary>
    public string? PEPrac { get; set; }

    /// <summary>
    /// Gets or sets the total marks for Pediatrics.
    /// </summary>
    public string? PETotal { get; set; }

    /// <summary>
    /// Gets or sets the result for Pediatrics.
    /// </summary>
    public string? PEResult { get; set; }

    #endregion
}