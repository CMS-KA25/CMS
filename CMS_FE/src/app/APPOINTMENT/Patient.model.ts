export interface Patient {
    date_of_birth: string;
    sex: string;
    country: string;
    pincode: string;
    city: string;
    state: string;
    address: string;
    marital_status: string;
    blood_group: string;
    allergies: string;
    chief_medical_complaints: string;
    consulted_before: boolean;
    doctor_name: string;
    last_review_date: string;
    seeking_followup: boolean;
    profile_image_path: File | null;
    medical_reports_path: File | null;
}