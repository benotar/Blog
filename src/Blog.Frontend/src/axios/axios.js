import axios from "axios";

const $axios = axios.create({
    baseURL: "/api",
    withCredentials: true,
    headers: {
        "Content-Type": "application/json"
    }
});

export default $axios;