using System;

namespace MedsProcessor.Common
{
	public enum DrugListTypeFilter
	{
		All = DrugListType.Undefined,
		Primary = DrugListType.Primary,
		Supplementary = DrugListType.Supplementary
	}

	public enum ProcessorState
	{
		NotRan,
		Ran,
		DataCleared,
		Running
	}

	public enum DrugListType
	{
		Undefined = 0,
		Primary = 1,
		Supplementary = 2
	}

	/// <remarks>
	/// O		oralno
	/// P		parenteralno
	/// R		rektalno
	/// N		nazalno
	/// L		lokalno
	/// SL	sublingvalno
	/// TD	transdermalno
	/// V		vaginalno
	/// I		inhalacija
	/// </remarks>
	[Flags]
	public enum DrugApplicationType
	{
		Undefined = 0,
		Oral = 1,
		Parenteral = 2,
		OralParenteral = Oral | Parenteral,
		Rectal = 4,
		Nazal = 8,
		Local = 16,
		SubLingvinal = 32,
		TransDermal = 64,
		Vaginal = 128,
		Inhaled = 256,
	}

	[Flags]
	public enum DrugApplicationTypeShort
	{
		Undefined = 0,
		O = DrugApplicationType.Oral,
		P = DrugApplicationType.Parenteral,
		OP = DrugApplicationType.OralParenteral,
		R = DrugApplicationType.Rectal,
		N = DrugApplicationType.Nazal,
		L = DrugApplicationType.Local,
		SL = DrugApplicationType.SubLingvinal,
		TD = DrugApplicationType.TransDermal,
		V = DrugApplicationType.Vaginal,
		I = DrugApplicationType.Inhaled
	}

	/// <remarks>
	/// R		izdavanje na recept
	///	RS	izdavanje na recept po preporuci specijalista
	/// RSf	izdavanje pripravaka za potrebe liječenja fenilketonurije, po preporuci bolničkog specijalista
	/// </remarks>
	public enum DrugPrescriptionType
	{
		Unprescribed = 0,
		Prescribed = 1,
		PrescribedBySpecialist = 2,
		PrescribedBySpecialistForPhenylketonuria = 3
	}

	public enum DrugPrescriptionTypeShort
	{
		BR = DrugPrescriptionType.Unprescribed,
		R = DrugPrescriptionType.Prescribed,
		RS = DrugPrescriptionType.PrescribedBySpecialist,
		RSf = DrugPrescriptionType.PrescribedBySpecialistForPhenylketonuria
	}

	[Flags]
	public enum DrugApplicationTypeLimitation
	{
		Undefined = 0,
		ClinicsOnly = 1,
		ClinicsAndOtherStationaries = 2,
		ClinicsOrClinicsAndOtherStationaries = ClinicsOnly | ClinicsAndOtherStationaries,
		OtherStationaries = 4,
		ClinicsAndOtherStationariesOrOtherStationaries = ClinicsAndOtherStationaries | OtherStationaries,
		ClinicsOrOtherStationaries = ClinicsAndOtherStationaries | OtherStationaries,
		PrimaryHealthcareInstitutions = 8,
		PrimaryHealthcareInstitutionsWithSubsidaryPayments = 16,
		ClinicsAndOtherStationariesOrPHIWSP = ClinicsAndOtherStationaries | PrimaryHealthcareInstitutionsWithSubsidaryPayments,
		OtherStationariesOrPHIWSP = OtherStationaries | PrimaryHealthcareInstitutionsWithSubsidaryPayments,
		MedicalDoctorsPrescriptionOnly = 32,
		OtherStationariesOrMDP = OtherStationaries | MedicalDoctorsPrescriptionOnly,
		MedicalDoctorsPrescriptionBasedOnIndications = 64,
		ByHzzoInCaseOfBondagesForHomecare = 128,
	}

	[Flags]
	public enum DrugApplicationTypeLimitationShort
	{
		Undefined = DrugApplicationTypeLimitation.Undefined,
		KL = DrugApplicationTypeLimitation.ClinicsOnly,
		KS = DrugApplicationTypeLimitation.ClinicsAndOtherStationaries,
		DS = DrugApplicationTypeLimitation.OtherStationaries,
		PR = DrugApplicationTypeLimitation.PrimaryHealthcareInstitutions,
		PO = DrugApplicationTypeLimitation.PrimaryHealthcareInstitutionsWithSubsidaryPayments,
		RL = DrugApplicationTypeLimitation.MedicalDoctorsPrescriptionOnly,
		XX = DrugApplicationTypeLimitation.MedicalDoctorsPrescriptionBasedOnIndications,
		RZ = DrugApplicationTypeLimitation.ByHzzoInCaseOfBondagesForHomecare,
		DSPO = DrugApplicationTypeLimitation.OtherStationariesOrPHIWSP,
		DSRL = DrugApplicationTypeLimitation.OtherStationariesOrMDP,
		KLDS = DrugApplicationTypeLimitation.ClinicsOrOtherStationaries,
		KSDS = DrugApplicationTypeLimitation.ClinicsAndOtherStationariesOrOtherStationaries,
		KLKS = DrugApplicationTypeLimitation.ClinicsOrClinicsAndOtherStationaries,
		KSPO = DrugApplicationTypeLimitation.ClinicsAndOtherStationariesOrPHIWSP
	}
}