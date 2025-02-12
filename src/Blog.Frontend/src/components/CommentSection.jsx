import {useAppStore} from "../zustand/useAppStore.js";
import {Link} from "react-router-dom";
import {Alert, Button, Textarea} from "flowbite-react";
import {useEffect, useState} from "react";
import $axios from "../axios/axios.js";
import Comment from "./Comment.jsx";


const COMMENT_MAX_LENGTH = 200;

const CommentSection = ({postId}) => {
    const currentUser = useAppStore(state => state.currentUser);
    const [comment, setComment] = useState("");
    const [commentError, setCommentError] = useState(null);
    const [comments, setComments] = useState([]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (comment.length > 200) {
            return;
        }

        try {
            const {data} = await $axios.post(`comment/create-comment/${currentUser.id}`, {
                postId,
                content: comment
            });

            if (!data.isSucceed) {
                setCommentError(data.errorCode);
                return;
            }

            setCommentError(null);
            setComment('');
        } catch (error) {
            setCommentError(error.message);
        }
    }

    useEffect(() => {
        const fetchComments = async () => {
            try {
                const {data} = await $axios.get(`comment/get-comments/${postId}`);

                if (!data.isSucceed) {
                    setCommentError(data.errorCode);
                    return;
                }

                setCommentError(null);
                setComments(data.payload);
            } catch (error) {
                console.log(error.message);
            }
        }

        fetchComments();
    }, [postId]);


    return (
        <div className="max-w-2xl mx-auto w-full p-3">
            {
                currentUser
                    ? (
                        <div className="flex items-center gap-1 my-5 text-gray-500 text-sm">
                            <p>Signed in as:</p>
                            <img
                                className="h-5 w-5 object-cover rounded-full"
                                src={currentUser.profilePictureUrl}
                                alt="profile-image"/>
                            <Link
                                className="text-xs text-cyan-500 hover:underline"
                                to={"/dashboard?tab=profile"}
                            >
                                @{currentUser.username}
                            </Link>
                        </div>
                    )
                    : (
                        <div className="flex gap-1 text-sm text-teal-500 my-5">
                            You must be signed in to comment.
                            <Link
                                className="text-blue-500 hover:underline"
                                to="/sign-in">
                                Sign In
                            </Link>
                        </div>
                    )
            }
            {
                currentUser && (
                    <form
                        onSubmit={handleSubmit}
                        className="border border-teal-500 rounded-md p-3"
                    >
                        <Textarea
                            placeholder="Add a comment..."
                            rows={3}
                            maxLength={COMMENT_MAX_LENGTH}
                            onChange={(e) => setComment(e.target.value)}
                            value={comment}
                        />
                        <div className="flex justify-between items-center mt-5">
                            <p className="text-gray-500 text-xs">
                                {COMMENT_MAX_LENGTH - comment.length} characters remaining
                            </p>
                            <Button
                                outline
                                gradientDuoTone="purpleToBlue"
                                type="submit"
                            >
                                Submit
                            </Button>
                        </div>
                        {commentError && (
                            <Alert
                                className="mt-5"
                                color="failure"
                            >
                                {commentError}
                            </Alert>
                        )}
                    </form>
                )
            }
            {comments.length === 0 ? (
                <p className="text-sm my-5">No comments yet!</p>
            ) : (
                <>
                    <div className="flex items-center gap-1 text-sm my-5">
                        <p>Comments</p>
                        <div className="border border-gray-400 py-1 px-2 rounded-sm">
                            <p>{comments.length}</p>
                        </div>
                    </div>
                    {
                        comments.map(comment => (
                            <Comment
                                key={comment.id}
                                comment={comment}
                            />
                        ))
                    }
                </>

            )}
        </div>
    );
}

export default CommentSection;