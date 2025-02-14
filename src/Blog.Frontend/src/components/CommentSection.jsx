import {useAppStore} from "../zustand/useAppStore.js";
import {Link} from "react-router-dom";
import {Alert, Button, Modal, Textarea} from "flowbite-react";
import {useEffect, useState} from "react";
import $axios from "../axios/axios.js";
import Comment from "./Comment.jsx";
import {useNavigate} from "react-router-dom";
import {HiOutlineExclamationCircle} from "react-icons/hi";

const COMMENT_MAX_LENGTH = 200;

const CommentSection = ({postId}) => {
    const currentUser = useAppStore(state => state.currentUser);
    const [comment, setComment] = useState("");
    const [commentError, setCommentError] = useState(null);
    const [comments, setComments] = useState([]);
    const navigate = useNavigate();
    const [currentPage, setCurrentPage] = useState(1);
    const [commentsPerPage] = useState(6);
    const [hasNextPage, setHasNextPage] = useState(false);
    const [hasPreviousPage, setHasPreviousPage] = useState(false);
    const [showModal, setShowModal] = useState(false);
    const [commentToDelete, setCommentToDelete] = useState(null);


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
            setComments([data.payload, ...comments]);
        } catch (error) {
            setCommentError(error.message);
        }
    }

    useEffect(() => {
        const fetchComments = async () => {
            try {
                const {data} = await $axios.get(`comment/get-comments/${postId}?page=${currentPage}&pageSize=${commentsPerPage}`);

                if (!data.isSucceed) {
                    setCommentError(data.errorCode);
                    return;
                }

                setCommentError(null);
                setComments(data.payload.items);
                setHasNextPage(data.payload.hasNextPage);
                setHasPreviousPage(data.payload.hasPreviousPage);
            } catch (error) {
                console.log(error.message);
            }
        }

        fetchComments();
    }, [postId, currentPage]);


    const handleLike = async (commentId) => {
        try {
            if (!currentUser) {
                navigate("sign-in");
                return;
            }

            const {data} = await $axios.put(`comment/like-comment/${commentId}`);

            if (data.isSucceed) {
                setComments(
                    comments.map((cm) =>
                        cm.id === commentId
                            ? {
                                ...cm,
                                likes: data.payload.likes,
                                countOfLikes: data.payload.countOfLikes
                            }
                            : cm
                    )
                );
            }
        } catch (error) {
            console.log(error.message);
        }
    }

    const handleEdit = async (comment, editedContent) => {
        setComments(comments.map((c) =>
                c.id === comment.id
                    ? {...c, content: editedContent}
                    : c
            )
        );
    };

    const handleDelete = async (commentId) => {
        setShowModal(false);

        try {
            if (!currentUser) {
                navigate("sign-in");
                return;
            }

            const {data} = await $axios.delete(`comment/delete-comment/${commentId}`);

            if (data.isSucceed) {
                setComments(
                    comments.filter((c) => c.id !== commentId)
                );
            }
        } catch (error) {
            console.log(error.message);
        }
    }

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
            <div className="flex mt-5 gap-x-5 items-center">
                <Button
                    size="xs"
                    outline
                    gradientDuoTone="greenToBlue"
                    disabled={!hasPreviousPage}
                    onClick={() => {
                        if (hasPreviousPage) {
                            setCurrentPage(prev => prev - 1);
                        }
                    }}
                >
                    Prev
                </Button>
                <span
                    className="font-bold mr-1 text-xl truncate"
                >{currentPage}
                </span>

                <Button
                    size="xs"
                    outline
                    gradientDuoTone="greenToBlue"
                    disabled={!hasNextPage}
                    onClick={() => {
                        if (hasNextPage) {
                            setCurrentPage(prev => prev + 1);
                        }
                    }}
                >
                    Next
                </Button>
            </div>
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
                                onLike={handleLike}
                                onEdit={handleEdit}
                                onDelete={(commentId) => {
                                    setShowModal(true)
                                    setCommentToDelete(commentId)
                                }}
                            />
                        ))
                    }
                </>

            )}
            <Modal
                show={showModal}
                onClose={() => setShowModal(false)}
                popup
                size="md"
            >
                <Modal.Header/>
                <Modal.Body>
                    <div className="text-center">
                        <HiOutlineExclamationCircle
                            className="h-14 w-14 text-gray-400 dark:text-gray-200
                            mb-4 mx-auto"
                        />
                        <h3
                            className="mb-5 text-lg text-gray-500 dark:text-gray-400"
                        >
                            Are you sure you want to delete this record?
                        </h3>
                        <div className="flex justify-center gap-4">
                            <Button
                                color="failure"
                                onClick={() => handleDelete(commentToDelete)}
                            >
                                Yes, I&apos;m sure
                            </Button>
                            <Button
                                color="gray"
                                onClick={() => setShowModal(false)}
                            >
                                No, cancel
                            </Button>
                        </div>
                    </div>
                </Modal.Body>
            </Modal>
        </div>
    );
}

export default CommentSection;