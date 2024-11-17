import axiosIntance from './Interceptor';

export function register(username,email,password,department){
    return axiosIntance.post('/api/User/register',{
          "userName": username,
            "email": email,
            "password": password,
            "department": +department
    })
}

export function login(email,password){
    return axiosIntance.post('/api/User/Login',{
        "email": email,
        "password": password
    })
}

export function getUserProfile()
{
    return axiosIntance.get('/api/User/profile')
}
