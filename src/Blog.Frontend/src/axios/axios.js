import axios from "axios";
import {useAppStore} from "../zustand/useAppStore.js";

const $axios = axios.create({
    baseURL: "/api",
    timeout: 10000,
    withCredentials: true,
    headers: {
        "Content-Type": "application/json"
    }
});

$axios.interceptors.request.use(config => {

    const storeState = useAppStore.getState();

    if (storeState.tokens?.accessToken) {
        const accessToken = storeState.tokens.accessToken;
        config.headers.Authorization = `Bearer ${accessToken}`;
    }

    return config;
}, error => {
    return Promise.reject(error);
});

export default $axios;