import {jwtDecode} from "jwt-decode";

const decodeJwt = (token) => {
    try {
        const decodedObject = jwtDecode(token);
        return {isAdmin: decodedObject.role === "Admin"};
    } catch(error) {
        console.error('Error decoding JWT token:', error);
        return null;
    }
}

export {decodeJwt};