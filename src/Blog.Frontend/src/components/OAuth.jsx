import {Button} from "flowbite-react";
import {AiFillGoogleCircle} from "react-icons/ai";
import {GoogleAuthProvider, signInWithPopup, getAuth} from "firebase/auth";
import {app} from "../firebase.js";
import $axios from "../axios/axios.js";
import {useAppStore} from "../zustand/useAppStore.js";
import {useNavigate} from "react-router-dom";

export default function OAuth() {

    const signInSuccess = useAppStore((state) => state.signInSuccess);
    const navigate = useNavigate();


    const handleGoogleClick = async () => {
        const provider = new GoogleAuthProvider();
        provider.setCustomParameters({prompt: "select_account"});
        try {
            const auth = getAuth(app);
            const resultFromGoogle = await signInWithPopup(auth, provider);
            const {data} = await $axios.post("auth/google", {
                name: resultFromGoogle.user.displayName,
                email: resultFromGoogle.user.email,
                googlePhotoURL: resultFromGoogle.user.photoURL
            });

            if (data.isSucceed === true) {
                signInSuccess(data.payload);
                navigate("/");
            }
        } catch (error) {
            console.log(error);
        }
    }
    return (
        <Button type="button" gradientDuoTone="pinkToOrange" outline onClick={handleGoogleClick}>
            <AiFillGoogleCircle className="w-6 h-6 mr-2"/>
            Continue with Google
        </Button>
    );
}


