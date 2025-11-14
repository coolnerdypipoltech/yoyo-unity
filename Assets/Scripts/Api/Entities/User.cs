using System;

[Serializable]
public class User
{
    public int id;
    public string name;
    public string email;
    public string email_verified_at;
    public string related_id;
    public string related_type;
    public string created_at;
    public string updated_at;
    public related related;
    public string avatar;
}

[Serializable]
public class related
{
    public int id;
    public int age;
    public string gender;
    public string phone;
    public int points;
    public int total_points;
    public string pronouns;
    public string access_code_id;
    public string taste_drink;
    public string taste_music;
    public string taste_food;
    public Gallery image;
    public Medium media;

}

[System.Serializable]
public class ImageResponse
{
    public Gallery image;
}

[Serializable]
public class SignInRequest
{
    public string name;
    public string email;
    public int age;
    public string gender;
    public string phone;
    public string password;
    public int points;
    public string pronouns;
    public string access_code;
}

[Serializable]
public class UserInfo
{
    public User user;
}

[Serializable]
public class LoginRequest
{
    public string email;
    public string password;
}

[Serializable]
public class ResetPasswordRequest
{
    public string email;
}

[Serializable]
public class UploadImageRequest
{
    public string image; // Base64 encoded image string
}

[Serializable]
public class PointsResult
{
    public int points;
    public int total_points;
}

[Serializable]
public class UpdateUserRequest
{
    public string id;
    public string name;
    public int age;
    public string gender;
    public string phone;
    public string pronouns;
    public string taste_drink;
    public string taste_music;
    public string taste_food;
}

[Serializable]
public class DeleteUserRequest
{
    public string password;
}

[Serializable]
public class CheckAccessCodeRequest
{
    public string code;
}

[Serializable]
public class LoginResponse
{
    public User user;
    public string access_token;
    public string token_type;
}

[Serializable]
public class UpdateUserResponse
{
    public User user;
}

[Serializable]
public class ErrorResponse
{
    public string error_code;
}

