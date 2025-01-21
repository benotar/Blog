import axios from "axios";

const $axios = axios.create({
    baseURL: "/api",
    timeout: 10000,
    withCredentials: true,
    headers: {
        "Content-Type": "application/json"
    }
});

export default $axios;